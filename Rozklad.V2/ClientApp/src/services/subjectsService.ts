import {restApiUrl} from "../shared/urls";
import {SubjectDto} from "../models/Subject";

export class subjectsService {
    static async getSubjects(studentId: string, token: string): Promise<SubjectDto[]> {
        const url: string = `${window.location.protocol}//${window.location.host}/${restApiUrl}/student/${studentId}/subjects`
        const response = await fetch(
            url,
            {
                headers: {
                    "Authorization": `Bearer ${token}`
                }
            }
        );
        return (await response.json()) as SubjectDto[];
    }

    static async updateSubjects(studentId: string, subjects: string[], token: string) {
        const url: string = `${window.location.protocol}//${window.location.host}/${restApiUrl}/student/${studentId}`
        const body =
            [
                {
                    "op": "add",
                    "path": "/Subjects",
                    "value": subjects
                }
            ]

        const result = await fetch(url, {
            method: "PATCH",
            body: JSON.stringify(body),
            headers: {
                Accept: "*/*",
                "Content-Type": "application/json",
                "Access-Control-Allow-Origin": "*",
                "Authorization": `Bearer ${token}`
            },
        });
        return result
    }

    static async getSubjectsToChoice(studentId: string) {
        const url: string = `${window.location.protocol}//${window.location.host}/${restApiUrl}/student/${studentId}/subject/choice`
        const response = await fetch(
            url
        );
        return (await response.json()) as SubjectDto[];
    }

    static async updateSubjectLinks(type: string, link: string, subjectId: string, token: string) {
        const url: string = `${window.location.protocol}//${window.location.host}/${restApiUrl}/subject/${subjectId}`
        const body =
            [
                {
                    "op": "add",
                    "path": type === "Лек" ? "/lessonsZoom" : "/labsZoom",
                    "value": link
                }
            ]
        const result = await fetch(
            url, {
                method: "PATCH",
                body: JSON.stringify(body),
                headers: {
                    Accept: "*/*",
                    "Content-Type": "application/json",
                    "Access-Control-Allow-Origin": "*",
                    "Authorization": `Bearer ${token}`
                }
            }
        )

        return result;
    }
    static async getDisabledSubjects(studentId : string, token :string):Promise<SubjectDto[]>{
        const url: string = `${window.location.protocol}//${window.location.host}/${restApiUrl}/student/${studentId}/subjects/disabled`
        const response = await fetch(
            url,
            {
                headers: {
                    "Authorization": `Bearer ${token}`
                }
            }
        );
        return (await response.json()) as SubjectDto[];
    }
    static async disableSubject(subjectId: string, studentId: string, token: string): Promise<boolean> {
        const url: string = `${window.location.protocol}//${window.location.host}/${restApiUrl}/student/${studentId}/subjects/${subjectId}/disable`
        const result = await fetch(url, {
            method: "PATCH",
            headers: {
                Accept: "*/*",
                "Content-Type": "application/json",
                "Access-Control-Allow-Origin": "*",
                "Authorization": `Bearer ${token}`
            }
        })

        return result.ok

    }

    static async enableSubject(subjectId: string, studentId: string, token: string): Promise<boolean> {
        const url: string = `${window.location.protocol}//${window.location.host}/${restApiUrl}/student/${studentId}/subjects/${subjectId}/enable`
        const result = await fetch(url, {
            method: "PATCH",
            headers: {
                Accept: "*/*",
                "Content-Type": "application/json",
                "Access-Control-Allow-Origin": "*",
                "Authorization": `Bearer ${token}`
            }
        })
        return result.ok


    }
}