import {TelegramUser} from "@v9v/ts-react-telegram-login";

export interface AuthRequestData{
    telegramUser: TelegramUser,
    group: string
}