import { CommonModule } from '@angular/common';
import { Component, HostListener } from '@angular/core';
import { Router, NavigationEnd } from '@angular/router';
import { CartService } from '../../../services/cart/cart.service';
import { AuthService } from '../../../services/auth/auth.service';
@Component({
  selector: 'app-header',
  imports: [CommonModule],
  templateUrl: './header.component.html',
  styleUrl: './header.component.css'
})
export class HeaderComponent {
  @HostListener('window:scroll', [])

  cartVisible = false;
  isScrolled = false;
  currentUrl: string = '';

  cartItems: any[] = [];
  totalPrice: number = 0;
  totalItemsCount: number = 0;

  constructor
  (
    private router: Router, 
    private cartService: CartService,
    private authService: AuthService,
  ) {
    this.router.events.subscribe(event => {
      if (event instanceof NavigationEnd) {
        this.currentUrl = event.urlAfterRedirects;
      }
    });
  }

  ngOnInit() {
    const customerId = this.authService.getUserIdFromToken();
    if(!customerId) return;

    this.cartService.getCart(customerId).subscribe(cart => {
      this.cartItems = cart.items;
      this.totalPrice = cart.items.reduce((acc, item) => acc + item.product!.price * item.quantity, 0);
    });
    // To fetch the item count
    this.cartService.getCartItemCount().subscribe(count => {
      this.totalItemsCount = count;
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

  
  showCart() {
    const userId = this.authService.getUserIdFromToken();
    if (!userId) return;

    this.cartService.getCart(userId).subscribe(cart => {
      this.cartItems = cart.items;
      this.totalPrice = cart.items.reduce((sum, item) => sum + item.product!.price * item.quantity, 0);
      this.cartVisible = true;
    });
  }

  hideCart() {
    this.cartVisible = false;
  }

}
