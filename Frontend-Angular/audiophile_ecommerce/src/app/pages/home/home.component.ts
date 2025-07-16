import { Component, AfterViewInit} from '@angular/core';
import { gsap } from 'gsap';
import {ScrollTrigger} from 'gsap/ScrollTrigger';
// import { AnimationOptions } from 'ngx-lottie';
import { CommonModule } from '@angular/common';

import { PromotionsComponent } from './promotions/promotions.component';
import { AboutComponent } from './about/about.component';

import { Router } from '@angular/router';
gsap.registerPlugin(ScrollTrigger);

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, PromotionsComponent, AboutComponent ],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent implements AfterViewInit{
  // @ViewChild('categoryCard', { static: false, read: ElementRef }) categoryCards!: ElementRef;

  constructor(private router: Router) {}

  ngAfterViewInit(): void {
    setTimeout(() => {
      gsap.from('.category-card', {
        y: 100,
        opacity: 0,
        duration: 1,
        stagger: 0.2,
        ease: 'power3.out',
        scrollTrigger: {
          trigger: '.category-container',
          start: 'top 80%',
          toggleActions: 'play none none reverse'
        }
      });
    });
  }


  categories = [
    {title: 'HEADPHONES', image:'headphones-image.png', },
    {title: 'SPEAKERS', image:'3D_Speaker-image.png', },
    {title: 'EARPHONES', image:'earphones-image.png', },
  ]

  navigateToCategory(category: string): void {
    this.router.navigate(['/category', category]);
  }


}
