import {restApiUrl} from "../shared/urls";


export async function GetGroups():Promise<string[]>{
    const url: string = `${restApiUrl}/groups`
    const result  = await fetch(url);
    return result.json();
}
