import { Component } from '@angular/core';
import {RouterOutlet} from '@angular/router';
import {MatCard} from '@angular/material/card';

@Component({
  imports: [
    RouterOutlet,
    MatCard
  ],
  selector: 'app-auth-layout',
  styleUrl: './auth-layout.scss',
  templateUrl: './auth-layout.html',
})
export class AuthLayout {}
