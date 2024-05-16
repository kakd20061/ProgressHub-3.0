export interface UserAdministrationModel {
  name: string;
  lastName: string;
  email: string;
  nickname: string;
  role: string;
  avatar: string;
  dateOfBirth:Date;
  gender: string;
  bodyParameters: bodyParameters;
}

interface bodyParameters {
  weight: string;
  height: string;
  bodyFatPercentage: string;
}
