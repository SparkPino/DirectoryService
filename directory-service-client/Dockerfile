#-берем готовый образ: Alpine Linux, в котором уже установлено Node.js 22. называем этап зависимости , переходим в app и даем команду копировать туда package.json package-lock.json указывая  ./ в конце ,где ./ это /app и вызываем рестор(npm ci)--
FROM node:22-alpine AS deps
WORKDIR /app
COPY package.json package-lock.json ./
RUN npm ci

# Этап build: снова чистый node:22-alpine, /app создаёт WORKDIR.
# node_modules берём из образа deps (там его поставил npm ci под Linux).
# Локальный node_modules (Windows, тяжёлый) исключён в .dockerignore,
# поэтому через COPY . . он не попадёт — остальной код копируем уже оттуда.
FROM node:22-alpine AS build
WORKDIR /app
COPY --from=deps /app/node_modules ./node_modules
COPY . .
# Значение приходит из docker-compose (build.args ← корневой .env).
# "/" — запасной дефолт для docker build без compose.
# Не "", потому что в коде роутинг без слеша ("api/departments") → з порожнім baseURL
ARG NEXT_PUBLIC_API_URL="/"
ENV NEXT_PUBLIC_API_URL=$NEXT_PUBLIC_API_URL
RUN npm run build

# ---------- 3. Запуск: тільки результат збірки ----------
FROM node:22-alpine AS runner
WORKDIR /app
ENV NODE_ENV=production
ENV PORT=3000
# слухати всі інтерфейси, а не тільки localhost контейнера
ENV HOSTNAME=0.0.0.0
COPY --from=build --chown=node:node /app/public ./public
COPY --from=build --chown=node:node /app/.next/standalone ./
COPY --from=build --chown=node:node /app/.next/static ./.next/static
# не root
USER node
EXPOSE 3000
CMD ["node", "server.js"]
