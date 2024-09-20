import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NotFoundComponent } from './components/errors/not-found/not-found.component';
import { ValidationMessagesComponent } from './components/errors/validation-messages/validation-messages.component';
import { ReactiveFormsModule } from '@angular/forms';
import { HTTP_INTERCEPTORS, HttpClient, HttpClientModule } from '@angular/common/http';
import { JwtInterceptor } from './interceptor/jwt.interceptor';
import { LoaderComponent } from './components/loader/loader.component';




@NgModule({
  declarations: [
    NotFoundComponent,
    ValidationMessagesComponent,
    LoaderComponent,
    
  ],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    HttpClientModule,
  ],
  providers: [
    { provide: HTTP_INTERCEPTORS, useClass: JwtInterceptor, multi: true }
  ],
  exports:[
    ReactiveFormsModule,
    HttpClientModule,
    ValidationMessagesComponent,
    HttpClientModule

  ]
})
export class SharedModule { }
