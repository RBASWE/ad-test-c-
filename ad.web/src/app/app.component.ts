import { HttpClient } from '@angular/common/http';
import { Component, ElementRef, ViewChild, inject } from '@angular/core';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
})
export class AppComponent {
  @ViewChild('username') username!: ElementRef;
  @ViewChild('password') password!: ElementRef;

  #http = inject(HttpClient);

  baseUrl = 'https://localhost:7239';

  onSubmit() {
    this.#http
      .post(this.baseUrl + '/login', {
        username: this.username.nativeElement.value,
        password: this.password.nativeElement.value,
      })
      .subscribe((data) => {
        console.log(data);
      });
  }
}
