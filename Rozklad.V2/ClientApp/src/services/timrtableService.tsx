import {restApiUrl} from "../shared/urls";
import {LessonWithSubject} from "../models/LessonWithSubject";

class TimetableService {
    static async getTimeTable(studentId: string, token: string): Promise<LessonWithSubject[]> {
        const url: string = `${restApiUrl}/student/${studentId}/timetable`
        const response = await fetch(
            url,
            {
                headers: {
                    "Authorization": `Bearer ${token}`
                }
            }
        );
        return (await response.json()) as LessonWithSubject[];
    }

    static async getGroupTimeTable(group: string): Promise<LessonWithSubject[]> {
        const url: string = `${restApiUrl}/groups/${group}/timetable`
        const response = await fetch(
            url
        );
        return (await response.json()) as LessonWithSubject[];
    }

    // static getTimeTable(studentId: string): LessonWithSubject[] {}
}

export default TimetableService;
