// @ts-ignore
import React from "react";


interface TelegramLoginButtonProps {
    botName: string
    dataOnauth?: Function
    buttonSize: "large" | "medium" | "small"
    cornerRadius: number,
    requestAccess: string,
    usePic: Boolean,
    dataAuthUrl? : string,
    lang?: string
}
interface Window {
    TelegramLoginWidget : any
}
class TelegramLoginButton extends React.Component {
    constructor(props: TelegramLoginButtonProps) {
        super(props);
        this.props = props
    }

    props: TelegramLoginButtonProps
    script : any
    componentDidMount() {

        const {
            botName, buttonSize,
            cornerRadius,
            requestAccess,
            usePic,
            dataOnauth,
            dataAuthUrl,
            lang,
        } = this.props
        
        // @ts-ignore
        window.TelegramLoginWidget = {
            dataOnAuth: (user : any) => dataOnauth!(user)
        }
        
        const script = document.createElement("script")
        script.src = "https://telegram.org/js/telegram-widget.js?14"
        script.setAttribute("data-telegram-login", botName);
        script.setAttribute("data-size", buttonSize);
        if (dataAuthUrl !== undefined) {
            script.setAttribute("data-auth-url", dataAuthUrl);
        } else {
            script.setAttribute(
                "data-onauth",
                "TelegramLoginWidget.dataOnAuth(user)"
            );
        }
        script.async = true;
        // this.instance.appendChild(script);
        // document.activeElement!.appendChild(script)
        (this as any).instance.appendChild(script)
        
    }
    
    render=()=>{
        
        return (
            <div  className={(this as any).props.className}
                  ref={(component) => {
                      (this as any).instance = component;
                  }}>
            </div>
        );
    }
}

export default TelegramLoginButton