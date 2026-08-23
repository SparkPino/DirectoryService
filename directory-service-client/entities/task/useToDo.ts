import { useState } from "react"

export type Task = {
    id: string
    text: string
    done: boolean
}
 
export default function useToDo() {

    const [tasks, setTasks] = useState<Task[]>([])

    function handleRemoveTask(id: string) {
        setTasks(t => t.filter((a) => a.id !== id))
    }
    
    function handleToggleTask(id: string) { 
       
        setTasks(t => t.map((task) => task.id === id ? { ...task, done: !task.done } : task))
    }

     function handleAddTask(message: string) {
    
         if (message.trim().length === 0) {
            return
      }
        setTasks(t => [...t, { id: crypto.randomUUID(), text: message, done: false }])
       
    }

    return  {removeTask: handleRemoveTask, toggleTask: handleToggleTask, addTask : handleAddTask, tasks}
}




      