import { Component, OnInit } from '@angular/core';
import { PlayService } from './play.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-play',
  templateUrl: './play.component.html',
  styleUrls: ['./play.component.css']
})
export class PlayComponent implements OnInit {
  constructor(private playService: PlayService,
    public toster:ToastrService
  ) {}
  
  message!: string;
  
  ngOnInit(): void {
    this.playService.getPlayers().subscribe({
      next: (response: any) => {
        this.message = response.message;
        this.toster.error(response.message);
      },
      error: (err: any) => console.log(err)
    });
  }
}
