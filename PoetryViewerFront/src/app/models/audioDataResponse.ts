export class AudioDataResponse {
    Name: string;
    AudioByteData: string;
    constructor(AudioByteData: string, Name: string){
        this.Name = Name;
        this.AudioByteData = AudioByteData;
    }
}