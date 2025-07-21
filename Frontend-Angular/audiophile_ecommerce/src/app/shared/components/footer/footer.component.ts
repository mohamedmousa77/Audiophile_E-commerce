import { Component, HostListener } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-footer',
  imports: [],
  templateUrl: './footer.component.html',
  styleUrl: './footer.component.css'
})
export class FooterComponent {

  constructor(private router: Router) {}

  navigateToCategory(category: string): void {
    this.router.navigate(['/category', category]);
  }
  navigateToHome(): void {
    this.router.navigate(['/'])
  }

}
