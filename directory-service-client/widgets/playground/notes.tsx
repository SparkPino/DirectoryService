"use client";

import { useState } from "react";
import { Button } from "@/shared/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/shared/ui/card";

type NoteSection = {
  heading: string;
  text: string;
  code?: string;
};

type Note = {
  id: string;
  label: string;
  title: string;
  sections: NoteSection[];
};

const notes: Note[] = [
  {
    id: "debounce",
    label: "useDebounce",
    title: "useDebounce — отложенное значение",
    sections: [
      {
        heading: "Что это",
        text: "Хук, который «догоняет» часто меняющееся значение с задержкой. Пока значение продолжает меняться, отложенная копия остаётся прежней. Когда изменений не было N миллисекунд, она принимает последнее значение.",
      },
      {
        heading: "Откуда берётся",
        text: "Это не часть React и не библиотек, которые уже есть в проекте (TanStack Query, react-hook-form и др.). Это отдельный пакет use-debounce, в проекте он пока не установлен. Такой же хук можно написать вручную через useState + useEffect + setTimeout (с clearTimeout в cleanup).",
        code: 'npm i use-debounce\n\nimport { useDebounce } from "use-debounce";',
      },
      {
        heading: "Как работает",
        text: "Каждое изменение value перезапускает таймер. Когда после последнего изменения проходит delay мс, отложенное значение обновляется. Хук возвращает кортеж [значение, controls], где controls содержит cancel, flush и isPending. Третьим аргументом принимает опции (leading, trailing, maxWait). Для функций есть useDebouncedCallback.",
        code: 'const [search, setSearch] = useState("");\nconst [debouncedSearch] = useDebounce(search, 500); // обновится через 500 мс после последней буквы',
      },
      {
        heading: "Для чего нужен",
        text: "Для поиска и фильтров, которые применяются сразу, без кнопки «Найти». Если положить debouncedSearch в ключ запроса TanStack Query, запрос уйдёт один раз после паузы в наборе, а не на каждую букву. Так меньше лишних запросов к серверу.",
        code: "const query = { Search: debouncedSearch || undefined };\nconst { data } = useLocationsList(query);",
      },
      {
        heading: "Нюансы",
        text: "• Откладывай только текстовое поле. Селекты и числа применяй сразу.\n• При постраничной пагинации сбрасывай Page в 1 при смене поиска (у infinite scroll запрос сам стартует заново).\n• placeholderData: keepPreviousData не даёт списку мигать спиннером при каждом изменении фильтра.\n• Блок синхронизации локального состояния из query (как в LocationFilter) будет затирать введённый текст, его нужно убрать.",
      },
    ],
  },
  {
    id: "state-manager",
    label: "State manager",
    title: "State manager (Zustand)",
    sections: [
      { heading: "Что это", text: "" },
      { heading: "Откуда берётся", text: "" },
      { heading: "Как работает", text: "" },
      { heading: "Для чего нужен", text: "" },
      { heading: "Нюансы", text: "" },
    ],
  },
];

export function PlaygroundNotes() {
  const [activeId, setActiveId] = useState(notes[0].id);
  const note = notes.find((n) => n.id === activeId) ?? notes[0];

  return (
    <section>
      <h2 className="text-xl font-semibold mb-4 text-center">Заметки</h2>

      <div className="flex justify-center gap-2 mb-4">
        {notes.map((n) => (
          <Button
            key={n.id}
            variant={n.id === note.id ? "default" : "outline"}
            aria-pressed={n.id === note.id}
            onClick={() => setActiveId(n.id)}
          >
            {n.label}
          </Button>
        ))}
      </div>

      <Card className="rounded-none ring-1 ring-foreground/15">
        <CardHeader>
          <CardTitle className="text-base font-bold">{note.title}</CardTitle>
        </CardHeader>
        <CardContent className="flex flex-col gap-4">
          {note.sections.map((section) => (
            <div key={section.heading} className="flex flex-col gap-1">
              <h3 className="font-semibold">{section.heading}</h3>
              {section.text ? (
                <p className="whitespace-pre-line">{section.text}</p>
              ) : (
                <p className="text-muted-foreground italic">
                  Здесь будет текст…
                </p>
              )}
              {section.code && (
                <pre className="bg-muted rounded-md p-3 text-xs overflow-x-auto">
                  <code>{section.code}</code>
                </pre>
              )}
            </div>
          ))}
        </CardContent>
      </Card>
    </section>
  );
}
