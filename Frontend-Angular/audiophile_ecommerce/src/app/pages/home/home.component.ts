import { Component } from '@angular/core';
import { gsap } from 'gsap';
import {ScrollTrigger} from 'gsap/ScrollTrigger';
import { AnimationOptions } from 'ngx-lottie';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-home',
  imports: [CommonModule],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent {
  earphonesAnimationOptions: AnimationOptions = {
    path: 'assets/animations/earphones.json'
  };

  // Animazione per le cuffie che girano (XX99)
  xx99AnimationOptions: AnimationOptions = {
    path: 'assets/animations/xx99.json',
    loop: true
  };

  // Animazione per lo speaker che sale nella sezione evidenziata
  speakerAnimationOptions: AnimationOptions = {
    path: 'assets/animations/speaker-reveal.json',
    loop: true
  };


}
