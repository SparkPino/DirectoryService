"use client"

export default function GlobalError({ error, reset: retry }: {
  error: Error & { digest?: string };
  reset: () => void;
}) {
  return (
    <html>
      <body style={{
        display: "flex",
        flexDirection: "column",
        alignItems: "center",
        justifyContent: "center",
        minHeight: "100vh",
        fontFamily: "sans-serif",
        gap: "16px",
        backgroundColor: "#f9fafb",
        margin: 0,
      }}>
        <h2 style={{ fontSize: "24px", fontWeight: "bold", color: "#111" }}>
          Критическая ошибка приложения
        </h2>
        {error.digest && (
          <p style={{ fontSize: "12px", color: "#999" }}>
            Код: {error.digest}
          </p>
        )}
        <button
          onClick={retry}
          style={{
            padding: "8px 20px",
            borderRadius: "6px",
            background: "#111",
            color: "#fff",
            cursor: "pointer",
            border: "none",
            fontSize: "14px",
          }}
        >
          Обновить страницу
        </button>
      </body>
    </html>
  );
}
