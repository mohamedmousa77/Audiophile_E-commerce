import { Component, ElementRef, Renderer2, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ProductService } from '../../services/product/product.service';
import { CommonModule } from '@angular/common';
import { Product } from '../../models/product';
import { CartService } from '../../services/cart/cart.service';
import { CartItemDTO } from '../../models/cartItemDTO';
import { AuthService } from '../../services/auth/auth.service';
import { AuthComponent } from '../../shared/components/modals/auth/auth.component';
import { FlyToCartDirective } from '../../shared/directives/fly-to-cart.directive';

@Component({
  selector: 'app-product-details',
  imports: [CommonModule, AuthComponent, FlyToCartDirective],
  templateUrl: './product-details.component.html',
  styleUrl: './product-details.component.css'
})

export class ProductDetailsComponent {
  @ViewChild('authModal') authComponent!: AuthComponent;
  @ViewChild(FlyToCartDirective) flyDirective!: FlyToCartDirective;
  // @ViewChild('productImage') productImage!: ElementRef;
  // @ViewChild('authModal') authModalRef: any;
  // @ViewChild(AuthComponent) authComponent!: AuthComponent;
  // @ViewChild('cartIcon', { read: ElementRef }) cartIcon!: ElementRef; // pass this via service or shared logic


  companyName = 'Audiophile';
  productsSuggested: Product[] = [];

  productId: number = 0;
  product: any;
  productQuantity = 1;
  

  constructor(
    private productService: ProductService,
    private cartService: CartService,
    private authService: AuthService,
    // private renderer: Renderer2,
    private router: Router,
  ) { }

  ngOnInit() {
    window.scrollTo({ top: 0, behavior: 'smooth' });

    const navigation = this.router.getCurrentNavigation();
    this.product = navigation?.extras?.state?.['product'];

    const storedProduct = sessionStorage.getItem('selectedProduct');
    if (storedProduct) {
      this.product = JSON.parse(storedProduct);
    }

    if(this.product){
      this.fetchSuggestedProducts(this.product.category);
    }
  }

  addToCart() {
    console.log(`add to cart called!`);

    if(!this.authService.getToken()){
      console.log(`User is not authenticated, there is no token: ${this.authService.getToken()}`);
      this.authComponent.showModal();
      // const modalEl = document.getElementById('authModal');
      // if (modalEl) {
      //   (modalEl as any).style.display = 'block';
      //   (modalEl as any).classList.add('show');
      // }
      return;
    }else {
      console.log(`Error while loading the token. Unalbe to authenticate`);
    }
    const customerId = this.authService.getUserIdFromToken();
    if(!customerId) return;

    const item: CartItemDTO = {
      productId: this.product.id,
      quantity: this.productQuantity
    };

    this.cartService.addToCart(customerId, item).subscribe({
      next: () => {
        // this.animateToCart(); // funzione opzionale
        this.flyDirective.fly();
        this.cartService.getCart(customerId).subscribe( cart => {
          const totalItemsCount = cart.items?.reduce((sum, item) => sum + item.quantity, 0) ||0;
          this.cartService.setCartItemCount(totalItemsCount);
        });
      },
      error: err => {
        console.error("Errore aggiunta al carrello", err);
      }
    });
  }

  // animateToCart() {
  // const productImg = document.getElementById('mainProductImg');
  // const cartIcon = document.getElementById('cartIcon');

  // if (!productImg || !cartIcon) return;

  // const clone = productImg.cloneNode(true) as HTMLElement;
  // const imgRect = productImg.getBoundingClientRect();
  // const cartRect = cartIcon.getBoundingClientRect();

  // clone.style.position = 'fixed';
  // clone.style.left = `${imgRect.left}px`;
  // clone.style.top = `${imgRect.top}px`;
  // clone.style.width = `${imgRect.width}px`;
  // clone.style.zIndex = '1000';
  // clone.style.transition = 'all 0.9s ease-in-out';

  // document.body.appendChild(clone);

  // requestAnimationFrame(() => {
  //   clone.style.left = `${cartRect.left}px`;
  //   clone.style.top = `${cartRect.top}px`;
  //   clone.style.opacity = '0.5';
  //   clone.style.transform = 'scale(0.2)';
  // });

  // setTimeout(() => { document.body.removeChild(clone); }, 1000);

  // }


  incrementQuantity(){
    this.productQuantity++;
  }

  discrementQuantity(){
    if (this.productQuantity > 1) {
      this.productQuantity--;
    }
  }

  viewProduct(product: Product) {
    window.scrollTo({ top: 0, behavior: 'smooth' });
    sessionStorage.setItem('selectedProduct', JSON.stringify(product));    
    this.router.navigate(['/product', product.id]);
  }

  fetchSuggestedProducts(categoryName: string): void {
    console.log(`Fetch products called. Product category is: ${this.product.category}`);
    this.productService.getProductsByCategory(categoryName)
    .subscribe(products => {
      this.productsSuggested = products;
      this.productsSuggested.forEach(element => {
        if(!element.imageUrl){
          element.imageUrl = 'place-holder-3.png';
        }
      });
      console.log(`Prodcuts fetched: ${this.productsSuggested}`);
    });      
  }
  

}
