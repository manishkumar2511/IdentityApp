import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class PlayService {
  constructor(private http: HttpClient) { }

  getPlayers() {
    return this.http.get('https://localhost:44385/api/Play/get-players');
  }
}
