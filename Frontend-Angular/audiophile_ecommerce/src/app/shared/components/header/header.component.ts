import { CommonModule } from '@angular/common';
import { Component, HostListener } from '@angular/core';
import { Router, NavigationEnd } from '@angular/router';
import { CartService } from '../../../services/cart/cart.service';
@Component({
  selector: 'app-header',
  imports: [CommonModule],
  templateUrl: './header.component.html',
  styleUrl: './header.component.css'
})
export class HeaderComponent {
  @HostListener('window:scroll', [])

  isScrolled = false;
  currentUrl: string = '';

  cartItems: any[] = [];
  total: number = 0;

  constructor
  (
    private router: Router, 
    private cartService: CartService,
  ) {
    this.router.events.subscribe(event => {
      if (event instanceof NavigationEnd) {
        this.currentUrl = event.urlAfterRedirects;
      }
    });
  }

  ngOnInit() {
    const customerId = 1;
    // this.authService.getUserId();
    this.cartService.getCart(customerId).subscribe(cart => {
      this.cartItems = cart.items;
      this.total = cart.items.reduce((acc, item) => acc + item.product!.price * item.quantity, 0);
    });
  }

  onWindowScroll() {
    this.isScrolled = window.scrollY > 50;
  }

  navigateToCategory(category: string): void {
    this.router.navigate(['/category', category]);
  }
  navigateToHome(): void {
    this.router.navigate(['/'])
  }

  isActiveCategory(category: string): boolean {
    const currentUrl = this.router.url;
    if (category === 'home') {
      return currentUrl === '/' || currentUrl === '/home';
    }
    return currentUrl.includes(`/category/${category}`);
  }

}
