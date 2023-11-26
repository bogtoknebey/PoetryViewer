import { Component, OnInit, Input, ChangeDetectorRef, HostListener } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { AudioDataResponse } from '../../models/audioDataResponse';

@Component({
  selector: 'app-player',
  templateUrl: './player.component.html',
  styleUrls: ['./player.component.css']
})
export class PlayerComponent implements OnInit{
  baseUrl:string = environment.baseUrl;
  @Input() author!: string;
  @Input() poetryNum!: string;
  pageCapacity: number = 3;
  currentPage: number = 1;
  audios: AudioDataResponse[] = [];
  sources: string[] = [];
  isLoad: boolean = false;

  counter: number = 0;
  constructor(private http: HttpClient, private cdr: ChangeDetectorRef){}
  ngOnInit(){
    this.reloadComponent();
  }
  
  reloadComponent() {
    this.getAudioRequest();
    console.log("changes are here-------------------------------------------");
    this.cdr.detectChanges();
  }

  getAudioRequest(){
    this.isLoad = true;
    this.http.get(`${this.baseUrl}/voice/${this.author}/${this.poetryNum}/${this.pageCapacity}/${this.currentPage}`)
    .subscribe(
      (response: any) => {
        this.audios = response;
        this.formSources();
        this.isLoad = false;
      },
      error => {
        this.audios = [];
        this.formSources();
        console.error('Error fetching data:', error);
        this.isLoad = false;
      }
    );
  }

  formSources(){
    this.counter++;
    this.sources = [];
    for(let i: number = 0; i < this.audios.length; i++){
      const audioData = this.audios[i].AudioByteData;

      const decodedAudioData = atob(audioData); // Decode base64 string
      const uint8Array = new Uint8Array(decodedAudioData.length);
      for (let j = 0; j < decodedAudioData.length; j++) {
        uint8Array[j] = decodedAudioData.charCodeAt(j);
      }

      const blob = new Blob([uint8Array], { type: 'audio/wav' });
      console.log('Blob size:', blob.size);
      this.sources.push(URL.createObjectURL(blob));

      console.log(`Audio with name ${this.audios[i].Name} was blobed...`);
    }
  }

  sourceAccessor(num: number): string{
    return this.sources[num];
  }

  deleteAudio(num: number){
    this.isLoad = true;
    let audioName = this.audios[num].Name;
    var r = confirm(`Are you sure that you want to delte audio: ${audioName}`);
    if (r == true) {
      alert("Ok. File will be deleted.");
      this.isLoad = false;
    }
    else {
      alert("Ok. File will not be deleted.");
      this.isLoad = false;
      return;
    }

    const formData = new FormData();
    formData.append('author', this.author);
    formData.append('poetryName', this.poetryNum);
    formData.append('audioName', audioName);

    this.http.post(`${this.baseUrl}/voice/delete`, formData)
    .subscribe(
      (response: any) => {
        console.log(`Seccessful delte response: ${response.Message}`);
        this.reloadComponent();
      },
      error => {
        console.error('Error fetching data:', error);
      }
    );
  }

  getPrevAudios(){
    if(this.currentPage > 1){
      this.currentPage--;
      this.reloadComponent();
    }
  }

  getNextAudios(){
    if(this.sources.length == this.pageCapacity){
      this.currentPage++;
      this.reloadComponent();
    }
  }

  @HostListener('document:keydown', ['$event'])
  handleKeyboardEvent(event: KeyboardEvent) {
    if (this.isLoad) return;
    if (event.key === 'ArrowLeft' || event.key === 'a' )
      this.getPrevAudios();
    if (event.key === 'ArrowRight' || event.key === 'd')
      this.getNextAudios();
  }
}
