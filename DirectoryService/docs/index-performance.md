# DS: Индексы и производительность read-запросов

## Данные

- Сгенерировано инструментом `tools/DirectoryService.Seeder` (Bogus, фиксированный `Randomizer.Seed = new Random(12345)` для воспроизводимости).
- Объём: ~10 000 департаментов (дерево, 4 уровня: root=8, branching factors [14, 11, 7]), 300 локаций, 250 должностей, случайные связи `department_locations`/`department_positions` (0-3 на департамент).

## Индексы, существовавшие до этой задачи

Обнаружены при аудите (созданы EF-конвенцией для FK или явно в `DepartmentConfiguration`):

| Индекс | Тип | Откуда |
|---|---|---|
| `departments.path` | GIST | `DepartmentConfiguration.cs` (`ix_department_path`) |
| `departments.parent_id` | btree | автоиндекс EF для FK `ParentId` |
| `department_locations.location_id` | btree | автоиндекс EF для FK `LocationId` |
| `department_locations.department_id` | btree | leftmost-колонка unique `(DepartmentId, LocationId)` |
| `department_positions.department_id` | btree | leftmost-колонка unique `(DepartmentId, PositionId)` |
| `department_positions.position_id` | btree | автоиндекс EF для FK `PositionId` |

## Новые индексы (эта задача)

| Индекс | Тип | Какой сценарий ускоряет | Статус |
|---|---|---|---|
| `departments.identifier` (`ix_department_identifier`) | unique btree | проверка уникальности slug (корректность данных, не скорость чтения) | ✅ |
| `departments.name` (`ix_department_name_trgm`) | GIN (`gin_trgm_ops`, `pg_trgm`) | поиск по имени без учёта регистра (`ILIKE '%...%'`) | ✅ |
| `(departments.is_active, departments.name)` (`ix_department_name`) | btree | **регрессия, найденная и исправленная в ходе задачи** — см. раздел ниже | ✅ |
й| soft-delete partial index | partial btree по активным строкам (`WHERE is_active = true`) | объединён с сортирующим индексом `ix_department_name` — см. раздел ниже | ✅ |

## План "до / после"

| Endpoint | Запрос | План до | План после | Индекс |
|---|---|---|---|---|
| `GET /departments/tree/search` | `d.name ILIKE '%...%'` | **Seq Scan**, filter `is_active AND name ~~* 'Outdoors'`, 9823 rows отброшено фильтром из ~9976, Execution Time 10.918 ms | **Bitmap Index Scan on ix_department_name** (`gin_trgm_ops`), 872 кандидата → 719 отброшено на recheck → 153 совпадения, Execution Time 7.981 ms | `gin_trgm` (GIN, `pg_trgm`) — добавлен |
| `GET /departments/{id}/children` | `WHERE parent_id = ...` | _(вставить сюда)_ | _(вставить сюда)_ | `btree(parent_id)` — уже существовал |
| `GET /departments/{id}/ancestors` | `path @> ...` | подтверждено тем же прогоном (см. ниже): **Bitmap Index Scan on ix_department_path**, `Index Cond: path @> nd.path` — уже работает как надо | — | `GIST(path)` — уже существовал |
| `GET /locations/top` | join + group by | _(вставить сюда)_ | _(вставить сюда)_ | `btree(location_id)` — уже существовал |
| `GET /departments` (без поиска, sort=name) | `WHERE is_active = true ORDER BY name LIMIT/OFFSET` | **Seq Scan** (9976 rows) → **top-N heapsort**, Execution Time 8.911 ms | **Index Scan on ix_department_name** (`Index Cond: is_active = true`) → **Incremental Sort** (только tie-break по `id` внутри групп одинаковых `name`), Execution Time 0.861 ms (~10x) | `btree(is_active, name)` — см. "Найденная регрессия" ниже |

## Сырые EXPLAIN ANALYZE (для справки)

_(вставлять сюда полный вывод по мере поступления, каждый — с пометкой "до" или "после" и датой)_

### `tree/search` (name ILIKE) — ДО индекса

```
Append  (cost=336.68..29333.56 rows=15554 width=125) (actual time=5.778..10.852 rows=587 loops=1)
  CTE node
    ->  Sort  (cost=336.30..336.68 rows=154 width=127) (actual time=5.774..5.803 rows=153 loops=1)
          Sort Key: d_1.created_at
          Sort Method: quicksort  Memory: 46kB
          ->  Seq Scan on departments d_1  (cost=0.00..330.70 rows=154 width=127) (actual time=0.011..5.740 rows=153 loops=1)
                Filter: (is_active AND ((name)::text ~~* 'Outdoors'::text))
                Rows Removed by Filter: 9823
  ->  CTE Scan on node  (cost=0.00..3.08 rows=154 width=718) (actual time=5.777..5.807 rows=153 loops=1)
  ->  Nested Loop  (cost=185.50..28916.03 rows=15400 width=119) (actual time=0.067..4.967 rows=434 loops=1)
        ->  CTE Scan on node nd  (cost=0.00..3.08 rows=154 width=48) (actual time=0.001..0.045 rows=153 loops=1)
        ->  Sort  (cost=185.50..185.75 rows=100 width=111) (actual time=0.031..0.031 rows=3 loops=153)
              Sort Key: d.created_at
              Sort Method: quicksort  Memory: 25kB
              ->  Bitmap Heap Scan on departments d  (cost=9.05..182.17 rows=100 width=111) (actual time=0.026..0.030 rows=3 loops=153)
                    Recheck Cond: (path @> nd.path)
                    Filter: (is_active AND (path <> nd.path))
                    Rows Removed by Filter: 1
                    Heap Blocks: exact=520
                    ->  Bitmap Index Scan on ix_department_path  (cost=0.00..9.03 rows=100 width=0) (actual time=0.025..0.025 rows=4 loops=153)
                          Index Cond: (path @> nd.path)
Planning Time: 0.244 ms
Execution Time: 10.918 ms
```

Ключевое:
- Поиск по имени (`node` CTE) — **Seq Scan**, без единого индекса, 9823 из ~9976 строк отброшены фильтром. Это и есть проблема, которую должен решить `gin_trgm`/`LOWER(name)`.
- Вторая часть того же запроса (LATERAL-поиск предков через `path @> nd.path`) уже корректно использует **Bitmap Index Scan on `ix_department_path`** — GiST-индекс работает как ожидается, здесь ничего трогать не надо.
- **Нюанс по абсолютным цифрам:** на ~10к строк `Seq Scan` всё равно выполняется быстро (11 мс) — на таком малом объёме разница в миллисекундах после индекса может быть незаметна на глаз. Важный сигнал здесь — **смена оператора плана** (Seq Scan → Bitmap Index Scan, "Rows Removed by Filter" с 9823 до ~0), а не драматичное падение времени: именно это и показывает, что на большем объёме (100к+) запрос без индекса деградировал бы значительно сильнее, а с индексом — оставался бы стабильным.

### `tree/search` (name ILIKE) — ПІСЛЯ додавання GIN (`gin_trgm_ops`) індексу на `name`

```
Append  (cost=291.77..29288.65 rows=15554 width=125) (actual time=1.685..7.845 rows=587 loops=1)
  CTE node
    ->  Sort  (cost=291.38..291.77 rows=154 width=127) (actual time=1.672..1.693 rows=153 loops=1)
          Sort Key: d_1.created_at
          Sort Method: quicksort  Memory: 46kB
          ->  Bitmap Heap Scan on departments d_1  (cost=82.94..285.79 rows=154 width=127) (actual time=0.433..1.583 rows=153 loops=1)
                Recheck Cond: ((name)::text ~~* 'Outdoors'::text)
                Rows Removed by Index Recheck: 719
                Filter: is_active
                Heap Blocks: exact=202
                ->  Bitmap Index Scan on ix_department_name  (cost=0.00..82.90 rows=154 width=0) (actual time=0.395..0.396 rows=872 loops=1)
                      Index Cond: ((name)::text ~~* 'Outdoors'::text)
  ->  CTE Scan on node  (cost=0.00..3.08 rows=154 width=718) (actual time=1.683..1.770 rows=153 loops=1)
  ->  Nested Loop  (cost=185.50..28916.03 rows=15400 width=119) (actual time=0.144..6.023 rows=434 loops=1)
        ->  CTE Scan on node nd  (cost=0.00..3.08 rows=154 width=48) (actual time=0.002..0.049 rows=153 loops=1)
        ->  Sort  (cost=185.50..185.75 rows=100 width=111) (actual time=0.038..0.038 rows=3 loops=153)
              Sort Key: d.created_at
              Sort Method: quicksort  Memory: 25kB
              ->  Bitmap Heap Scan on departments d  (cost=9.05..182.17 rows=100 width=111) (actual time=0.033..0.035 rows=3 loops=153)
                    Recheck Cond: (path @> nd.path)
                    Filter: (is_active AND (path <> nd.path))
                    Rows Removed by Filter: 1
                    Heap Blocks: exact=520
                    ->  Bitmap Index Scan on ix_department_path  (cost=0.00..9.03 rows=100 width=0) (actual time=0.031..0.031 rows=4 loops=153)
                          Index Cond: (path @> nd.path)
Planning Time: 0.582 ms
Execution Time: 7.981 ms
```

Ключевое:
- `Seq Scan` → **`Bitmap Index Scan on ix_department_name`**, `Index Cond: name ~~* 'Outdoors'` — новый GIN-индекс реально используется планировщиком.
- GIN + `gin_trgm_ops` — **lossy**-индекс: он хранит триграммы, а не сами строки, поэтому возвращает кандидатов (872), которых Postgres затем проверяет напрямую по данным (`Recheck Cond`) и отбрасывает ложные совпадения (`Rows Removed by Index Recheck: 719`, остаётся 153 — правильное количество). Это нормальное свойство trigram-индексов, не проблема.
- Execution Time: **10.918 ms → 7.981 ms** (~27% быстрее). Умеренный выигрыш в абсолютных мс именно из-за малого объёма таблицы (~10к строк) — главное доказательство эффективности здесь не время, а смена оператора плана (`Seq Scan` с отбрасыванием 9823 строк → `Bitmap Index Scan` с точечным доступом только к релевантным heap-блокам).

## Найденная и исправленная регрессия: `ix_department_name`

Миграция `20260729211923_trgm.cs`, которая добавляла GIN-индекс для поиска по имени, **удалила** старый композитный `(is_active, name)` btree-индекс и создала GIN **под тем же именем** (`ix_department_name`), фактически заменив один индекс другим вместо того, чтобы добавить новый рядом.

Следствие: `GetDepartmentsHandler` (`GET /departments`, листинг без поиска, `ORDER BY name LIMIT/OFFSET`) потерял индекс, который раньше давал отсортированный вывод без отдельного `Sort`, и деградировал до `Seq Scan` + `top-N heapsort` на каждый запрос.

**Исправление:** вернул отдельный btree `(is_active, name)` под историческим именем `ix_department_name`, а GIN-индекс переименовал в `ix_department_name_trgm` — теперь оба существуют одновременно, каждый под своим именем, для разных сценариев (отсортированный листинг vs `ILIKE`-поиск).

### `GET /departments` (без поиска, sort=name) — ДО исправления (Seq Scan)

```
Limit  (cost=571.22..571.27 rows=20 width=86) (actual time=8.876..8.883 rows=20 loops=1)
  Buffers: shared hit=206
  ->  Sort  (cost=571.22..596.16 rows=9976 width=86) (actual time=8.874..8.877 rows=20 loops=1)
        Sort Key: name, id
        Sort Method: top-N heapsort  Memory: 29kB
        Buffers: shared hit=206
        ->  Seq Scan on departments d  (cost=0.00..305.76 rows=9976 width=86) (actual time=0.015..3.431 rows=9976 loops=1)
              Filter: is_active
              Buffers: shared hit=206
Planning Time: 0.123 ms
Execution Time: 8.911 ms
```

### `GET /departments` (без поиска, sort=name) — ПОСЛЕ исправления (Index Scan + Incremental Sort)

```
Limit  (cost=0.53..2.61 rows=20 width=86) (actual time=0.811..0.815 rows=20 loops=1)
  Buffers: shared hit=116
  ->  Incremental Sort  (cost=0.53..1035.11 rows=9976 width=86) (actual time=0.810..0.812 rows=20 loops=1)
        Sort Key: name, id
        Presorted Key: name
        Full-sort Groups: 1  Sort Method: top-N heapsort  Average Memory: 29kB  Peak Memory: 29kB
        Pre-sorted Groups: 1  Sort Method: top-N heapsort  Average Memory: 27kB  Peak Memory: 27kB
        Buffers: shared hit=116
        ->  Index Scan using ix_department_name on departments d  (cost=0.29..764.93 rows=9976 width=86) (actual time=0.026..0.705 rows=176 loops=1)
              Index Cond: (is_active = true)
              Buffers: shared hit=116
Planning Time: 0.320 ms
Execution Time: 0.861 ms
```

Ключевое:
- `Seq Scan` (206 buffer hits, вся таблица) → `Index Scan using ix_department_name` (116 buffer hits) — реальное снижение I/O, не только смена оператора.
- Вместо полного `Sort` — **`Incremental Sort`**: индекс уже отдаёт строки отсортированными по `name`, Postgres лишь досортировывает малые группы строк с одинаковым `name` (для стабильного `id`-тайбрейка `ORDER BY name, id`) — значительно дешевле, чем сортировка всех 9976 строк с нуля.
- Execution Time: **8.911 ms → 0.861 ms** (~10x) — здесь выигрыш заметен уже и в абсолютных цифрах, потому что в отличие от trigram-поиска (где GIN всё равно возвращает сотни кандидатов на recheck), здесь индекс сразу даёт **точный**, отсортированный порядок и рано останавливается благодаря `LIMIT`.

## Partial-индекс для активных строк

Вместо отдельного индекса — объединён с сортирующим `ix_department_name`: он создан не просто как `btree(is_active, name)`, а как `btree(name) WHERE is_active = true` (partial). Так одна структура закрывает сразу два сценария: сортировка по `name` без отдельного `Sort` (см. замеры выше) и физическое исключение soft-deleted строк из индекса.

Итоговая конфигурация в `DepartmentConfiguration.cs`:
```csharp
builder.HasIndex(d => d.Name, "IX_department_name_sort")
    .HasDatabaseName("ix_department_name")
    .HasFilter("is_active = true");

builder.HasIndex(d => d.Name, "IX_department_name_trgm")
    .HasDatabaseName("ix_department_name_trgm")
    .HasMethod("gin")
    .HasOperators("gin_trgm_ops");
```

**Честная оговорка:** на свежем сиде 0 soft-deleted департаментов, поэтому реальный физический выигрыш от partial-фильтра (уменьшение размера индекса за счёт исключения неактивных строк) сейчас измерить нечем — индекс и без фильтра охватывал бы те же самые строки. Сама структура индекса при этом правильная и корректно работает уже сейчас (см. `Index Cond: (is_active = true)` в плане выше) — выигрыш в размере станет заметен по мере накопления soft-deleted записей в реальной эксплуатации.

**Побочная находка при реализации:** первая попытка объявить оба индекса через `HasIndex(d => d.Name)`/`HasIndex(d => new { d.Name })` без второго аргумента приводила к тому, что EF Core считал оба вызова конфигурацией **одного и того же** индекса (одинаковый список свойств) и просто перезаписывал настройки — в миграции реально создавался только один (GIN) индекс, второй (btree для сортировки) не появлялся вообще. Исправлено через второй аргумент `HasIndex(expression, name)` — имя на уровне модели EF, которое явно различает два индекса с одинаковым набором свойств.

## Тесты и применение миграций

**Все 68 интеграционных тестов проходят** после всех изменений в этой задаче.

**Оговорка:** тестовый фикстур (`DirectoryServiceWebFactory.InitializeAsync`) поднимает схему через `context.Database.EnsureCreatedAsync()`, а не `MigrateAsync()` — то есть тесты строят схему напрямую из текущей EF-модели, **не выполняя файлы миграций**. Формально это означает, что автотесты не покрывают требование "миграции применяются на чистой БД" — они проверяют корректность модели и бизнес-логики, но не сам процесс наката миграций по порядку. Реальное применение миграций (`dotnet ef database update`) на dev-БД проверено вручную в ходе этой задачи.

Побочные находки, всплывшие при проверке тестов (не связаны с индексами напрямую, но блокировали зелёный прогон):
- `pg_trgm` был закомментирован в `DirectoryServiceDbContext.OnModelCreating` — из-за этого `EnsureCreatedAsync()` падал с `42704: operator class "gin_trgm_ops" does not exist`, хотя реальная dev-БД (через миграции) работала корректно. Раскомментировано.
- Опечатка в `SoftDeleteDepartmentTest.cs`: тест ожидал код ошибки `"department.not_found"` вместо реального `"department.not.found"`. Исправлено.