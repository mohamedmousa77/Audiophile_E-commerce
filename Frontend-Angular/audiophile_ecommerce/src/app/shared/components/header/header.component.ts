import { CommonModule } from '@angular/common';
import { Component, HostListener } from '@angular/core';
import { Router, NavigationEnd } from '@angular/router';
@Component({
  selector: 'app-header',
  imports: [CommonModule],
  templateUrl: './header.component.html',
  styleUrl: './header.component.css'
})
export class HeaderComponent {
  isScrolled = false;
  currentUrl: string = '';


  @HostListener('window:scroll', [])
  
  onWindowScroll() {
    this.isScrolled = window.scrollY > 50;
  }

  constructor(private router: Router) {
    this.router.events.subscribe(event => {
      if (event instanceof NavigationEnd) {
        this.currentUrl = event.urlAfterRedirects;
      }
    });
  }


  navigateToCategory(category: string): void {
    this.router.navigate(['/category', category]);
  }
  navigateToHome(): void {
    this.router.navigate(['/'])
  }

  isActiveCategory(category: string): boolean {
    const currentUrl = this.router.url;
    console.log(`category: ${category}. Current url: ${currentUrl}`)
    if (category === 'home') {
      return currentUrl === '/' || currentUrl === '/home';
    }
    return currentUrl.includes(`/category/${category}`);
  }
}
