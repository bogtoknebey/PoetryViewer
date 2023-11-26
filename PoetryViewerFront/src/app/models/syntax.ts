export class Syntax {
    Author: string;
    Inclusions: { [key: string]: number };

    constructor(Author: string, Inclusions: { [key: string]: number }){
        this.Author = Author;
        this.Inclusions = Inclusions;
    }
    
}