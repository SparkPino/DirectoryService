"use client"

import { Button } from "@/shared/ui/button"
import { useEffect } from "react"

export default function Error({ error, reset: retry }: { error: Error & { digest?: string }; reset: () => void }) {
  useEffect(() => {
    console.error(error)
  }, [error])

  return (
    <div className="flex flex-col items-center justify-center min-h-screen gap-4 px-4 text-center">
      <div className="text-5xl">⚠️</div>
      <h2 className="text-2xl font-semibold">Что-то пошло не так</h2>
      <p className="text-muted-foreground text-sm max-w-md">{error.message}</p>
      {error.digest && (
        <p className="text-xs text-muted-foreground">Код: {error.digest}</p>
      )}
      <Button onClick={retry}>Повторить</Button>
    </div>
  )
}
