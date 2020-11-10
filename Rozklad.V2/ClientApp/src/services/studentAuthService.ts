import {StudentDto} from "../models/Student";
import {restApiUrl} from "../shared/urls";
import {TelegramUser} from "@v9v/ts-react-telegram-login";
import {AuthRequestData} from "../models/AuthRequestData";
import {InvalidDataError} from "../models/InvalidDataError";

export enum UsageTypes {
    Authentificated,
    ByGroup,
    Unauthentificated
}

class StudentAuthService {
    private token: string | null = null
    private group: string | null = null
    private student: StudentDto | null = null

    // todo maybe better to save student too 
    constructor() {
        const token = localStorage.getItem("token")
        const group = localStorage.getItem("group")
        const student : StudentDto= JSON.parse(localStorage.getItem("student")!)
        if(token !==null){
            this.setToken(token)
        }
        if(group !== null){
            this.setGroup(group)
        }
        if(student !== null){
            this.setStudent(student)
        }
    }
    groupLogin = (group:string) =>{
        this.setGroup(group)
        localStorage.setItem("group",group)    
    }
    async login(studentData: AuthRequestData) {
        const response = await fetch(`${window.location.protocol}//${window.location.host}/${restApiUrl}/student/telegram`, {
            method: "POST",
            headers: {
                Accept: "*/*",
                "Content-Type": "application/json",
                "Access-Control-Allow-Origin": "*",
            },
            body: JSON.stringify(studentData) || null
        });
        if (response.status === 400) {
            // info not valid 
            throw new InvalidDataError(await response.json())
        }
        if (response.status !== 200) {
            // error here  
            throw new Error(await response.json())
        }
        const student: StudentDto = await response.json()
        await localStorage.setItem("student", JSON.stringify(student))
        await localStorage.setItem("token", student.token)
        // todo group will be null if, token available
        return student
    }

    logout() {
        this.setGroup(null)
        this.setToken(null)
        this.setStudent(null)
        localStorage.clear()
    }

    isAuthentificated(): boolean {
        return !!this.token;
    }

    setToken(token: string | null) {

        this.token = token

    }
    
    getToken() {
        return this.token
    }

    setGroup(group: string | null) {

        this.group = group

    }

    getGroup() {
        return this.group
    }
    setStudent(st : StudentDto | null){
        this.student = st
    }
    getStudent(){
        return this.student
    }
    getUsageType(): UsageTypes {
        if (this.isAuthentificated()) {
            return UsageTypes.Authentificated
        } else if (this.getGroup()) {
            return UsageTypes.ByGroup
        } else {
            return UsageTypes.Unauthentificated
        }

    }

}


// class StudentAuthService {
//   private student_id: string | null = null;
//   constructor() {
//     const studentId = localStorage.getItem("student");
//     if (studentId != null) {
//       this.setStudentId(studentId);
//     }
//   }
//   async login(studentData: StudentFormData): Promise<{ id: string }> {
//     const { student_id } = await this.loginRequest(studentData);
//     await localStorage.setItem("student_id", student_id);
//     this.setStudentId(student_id);
//     return { id: student_id };
//   }
//
//   isAuthentificated(): boolean {
//     return !!this.student_id;
//   }
//
//   logout() {
//     this.setStudentId(null);
//     localStorage.clear();
//   }
//
//   private async getStudent(id: string): Promise<StudentDto> {
//     const response = await fetch(`${restApiUrl}/students/${id}`);
//     return (await response.json()) as StudentDto;
//   }
//
//   private async loginRequest(
//     studentData: StudentFormData
//   ): Promise<{ student_id: string }> {
//     const response = await fetch(`${restApiUrl}/student/login`, {
//       method: "POST",
//       headers: {
//         Accept: "*/*",
//         "Content-Type": "application/json",
//         "Access-Control-Allow-Origin": "*",
//       },
//       body: JSON.stringify(studentData) || null,
//     });
//     return (await response.json()) as { student_id: string };
//   }
//
//   private setStudentId(id: string | null) {
//     this.student_id = id;
//   }
//
//   getStudentID(): string {
//     return this.student_id!;
//   }
// }

export default StudentAuthService;
