export interface SubjectDto {
  id: string;
  name: string;
  teachers: string;
  lessonsZoom: string;
  labsZoom: string;
  isRequired: boolean;
}

export interface LinkInfo {
  url: string;
  accessCode: string;
}
