export class AudioRecord {
    Author: string;
    PoetryNum: number;
    AudioData: Blob;
    constructor(Author: string, PoetryNum: number, AudioData: Blob){
        this.Author = Author;
        this.PoetryNum = PoetryNum;
        this.AudioData = AudioData;
    }
}