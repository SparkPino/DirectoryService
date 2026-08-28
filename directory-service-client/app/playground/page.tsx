"use client"
import { TextareaButton } from "@/widgets/text-area-button";
import { Card, CardAction, CardContent } from "@/shared/ui/card";
import { useState } from "react";
import { Button } from "@/shared/ui/button";
import { Checkbox } from "@/shared/ui/checkbox";
import { Badge } from "@/shared/ui/badge";
import useToDo from "@/entities/task/useToDo";


export default function Playground() {

    
    const [message, setMessage] = useState<string>("")
    const { removeTask: handleRemoveTask, toggleTask: handleToggleTask, addTask: handleAddTask, tasks } = useToDo()

    function handleSubmit(text: string) {
        handleAddTask(text)
        setMessage("")
    }

    return (
        <div className="max-w-xl mx-auto py-10 px-4">
            <h1 className="text-2xl font-semibold mb-6 text-center">Мои задачи</h1>

            <TextareaButton setMessage={setMessage} onSubmit={handleSubmit} message={message} />

            <p className="text-sm text-muted-foreground mt-4 text-center">
                Осталось невыполненных: <span className="font-medium text-foreground">
                    {tasks.filter(t=> !t.done).length}</span>
            </p>

            <div className="flex flex-col gap-5 mt-4">
                {tasks.length === 0 && (
                    <p className="text-center text-muted-foreground py-8">Задач пока нет — добавьте первую!</p>
                )}

                {tasks.map((task) => (
                    <Card
                        key={task.id}
                        className={`rounded-none bg-muted/30 ring-1 ring-foreground/15 border-l-4 ${task.done ? "border-l-foreground" : "border-l-transparent"}`}
                    >
                        <CardContent className="relative flex items-center gap-2">
                            <Checkbox checked={task.done} onCheckedChange={() => handleToggleTask(task.id)} aria-label={task.text} />
                            <span className={`flex-1 text-center ${task.done ? "opacity-50 line-through" : ""}`}>{task.text}</span>
                            <Badge variant="outline" className="absolute top-1 right-2" title={task.id}>
                                {task.id.slice(0, 8)}
                            </Badge>
                        </CardContent>
                        <CardAction className="self-end">
                            <Button onClick={() => handleRemoveTask(task.id)}>
                                Удалить
                            </Button>
                        </CardAction>
                    </Card>
                ))}
            </div>
        </div>
    )
}



 