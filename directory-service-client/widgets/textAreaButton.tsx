import {Textarea} from "@/shared/ui/textarea"
import {Button} from "@/shared/ui/button"
import { Dispatch, SetStateAction } from "react"


type TextareaButtonProps = {
   message: string
  setMessage: Dispatch<SetStateAction<string>>
   onSubmit: (message: string) => void
}

 

export function TextareaButton({ setMessage , onSubmit ,message}: TextareaButtonProps) {
  return (
    <div className="grid w-full gap-2">
      <Textarea placeholder="Напиши задачу здесь."
      value={message} //где бы и как бы не поменялся message,тут это отобразиться в форме для текста.
      onChange={(m) => setMessage(m.target.value)} />  {/* onChange срабатывает при каждом изменении текста в поле — то есть буквально при каждом нажатии клавиши во время печати Кнопка "Отправить
  задание" тут вообще ни при чём */}
      <Button onClick={() => onSubmit(message)}>Отправить задание</Button>
    </div>
  )
}