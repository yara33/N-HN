import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { environment } from '../../environments/environment';

@Component({
  selector: 'app-hackernews',
  templateUrl: './hackernews.component.html',
  styleUrls: ['./hackernews.component.css']
})
export class HackernewsComponent implements OnInit {

  hnArray: [];
  searchWord: string;
  baseUrl = environment.apiUrl;

  constructor(private http: HttpClient, private router: Router) { }

  ngOnInit() {
    this.http.get(this.baseUrl)
      .subscribe((response: []) => {
        this.hnArray = response;
      });
  }

  search(searchWord) {
    this.http.get(this.baseUrl + searchWord )
      .subscribe((response: []) => {
        this.hnArray = response;
      });
  }

}
