export class PageResult<T> {

    constructor(
        public items: Array<T>,
        public totalCount: number      
        ){ }
}