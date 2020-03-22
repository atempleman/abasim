export interface User {
    id: number;
    username: string;
    passwordHash: any;
    passwordSalt: any;
    teamname: string;
    name: string;
    email: string;
    isAdmin: number;
}
