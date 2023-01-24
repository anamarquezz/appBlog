export class ApplicationUser {
    constructor (
        public applicationUserId: number,
        public username: string,   
        public fullname: string,  
        public token: string,
        public email: string,
    ) {
    }
}