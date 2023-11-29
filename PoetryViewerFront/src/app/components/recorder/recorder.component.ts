import { Component, OnInit, Input, HostListener } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { environment } from '../../environments/environment';

@Component({
  selector: 'app-recorder',
  templateUrl: './recorder.component.html',
  styleUrls: ['./recorder.component.css']
})

export class RecorderComponent implements OnInit {
  @Input() author: string = "";
  @Input() name: string = "";

  baseUrl:string = environment.baseUrl;
  private httpOptions: { headers: HttpHeaders, responseType: 'text' };

  audioChunks: Blob[] = [];
  mediaRecorder!: MediaRecorder;

  recording = false;
  hasRecord = false;
  sendedRecord = false;
  isLoad = false;

  constructor(private http: HttpClient) 
  {
    this.httpOptions = {
      headers: new HttpHeaders({
        'Content-Type': 'multipart/form-data',
      }),
      responseType: 'text',
    };
  }
  ngOnInit(){
    this.isLoad = true;
    this.setupRecording();
    this.isLoad = false;
  }


  setupRecording(){
    navigator.mediaDevices.getUserMedia({ audio: true })
    .then(stream => {
      this.mediaRecorder = new MediaRecorder(stream);
      this.mediaRecorder.ondataavailable = (event) => {
        if (event.data.size > 0) {
          this.audioChunks.push(event.data);
        }
      };
    })
    .catch(error => console.error('Error accessing microphone:', error));
  }
  startRecording() {
    this.audioChunks = [];
    this.mediaRecorder.start();
    this.recording = true;
    this.sendedRecord = false;
  }
  stopRecording() {
    this.mediaRecorder.stop();
    this.recording = false;
    this.hasRecord = true;

    console.log(this.audioChunks);
  }
  saveRecording() {
    this.sendedRecord = true;
    if (this.audioChunks.length === 0) return;

    const formData = new FormData();
    formData.append('Author', this.author);
    formData.append('PoetryNum', this.name);
    formData.append('AudioData', new Blob(this.audioChunks, { type: 'audio/wav' }));

    this.http.post(`${this.baseUrl}/voice`, formData)
      .subscribe(response => {
        console.log('Audio uploaded successfully:', response);
      });
  }
  
  @HostListener('document:keydown', ['$event'])
  handleKeyboardEvent(event: KeyboardEvent) {
    //this.recording
    if(this.isLoad) return;

    if(((event.ctrlKey || event.metaKey) && event.key == 's') || event.key === 'Enter'){
      event.preventDefault();
      if (this.hasRecord){
        this.saveRecording();
      }
    }
    if(event.key === ' '){
      event.preventDefault();
      if(this.recording){
        this.stopRecording();
      }else{
        this.startRecording();
      }
      
    }      
  }
}
