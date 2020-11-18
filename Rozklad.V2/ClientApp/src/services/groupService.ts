import {restApiUrl} from "../shared/urls";


export async function GetGroups():Promise<string[]>{
    const url: string = `${window.location.protocol}//${window.location.host}/${restApiUrl}/groups`
    const result  = await fetch(url)
    return result.json()
}
