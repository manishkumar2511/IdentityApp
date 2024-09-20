import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-rxjs-learning',
  templateUrl: './rxjs-learning.component.html',
  styleUrl: './rxjs-learning.component.css'
})
export class RxjsLearningComponent implements OnInit{
  //Observable decleared
  agent!:Observable<string>;
  agentName!:string;

ngOnInit(): void {
  //Observable created 
  this.agent=new Observable(
    function(observer){
      try{
        observer.next("Manish");
        setInterval(()=>{

        },3000);
       
        setInterval(()=>{
          observer.next("Kumar");
        },5000);
       
        setInterval(()=>{
          observer.next("Sita");
        },7000);
       

      }catch(e){
        observer.error(e);
      }
    }
  );
  this.agent.subscribe((data)=>{
    console.log(data);
    this.agentName=data;
  });
}

}
