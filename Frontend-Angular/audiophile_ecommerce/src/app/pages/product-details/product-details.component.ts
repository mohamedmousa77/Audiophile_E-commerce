import { Component, ElementRef, Renderer2, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ProductService } from '../../services/product/product.service';
import { CommonModule } from '@angular/common';
import { Product } from '../../models/product';
import { CartService } from '../../services/cart/cart.service';
import { CartItemDTO } from '../../models/cartItemDTO';
import { AuthService } from '../../services/auth/auth.service';

// import { NgbModal } from '@ng-bootstrap/ng-bootstrap';



@Component({
  selector: 'app-product-details',
  imports: [CommonModule],
  templateUrl: './product-details.component.html',
  styleUrl: './product-details.component.css'
})

export class ProductDetailsComponent {
  @ViewChild('productImage') productImage!: ElementRef;
  @ViewChild('authModal') authModalRef: any;
  @ViewChild('cartIcon', { read: ElementRef }) cartIcon!: ElementRef; // pass this via service or shared logic

  productId: number = 0;
  product: any;
  companyName = 'Audiophile';
  productQuantity = 1;
  productsSuggested: Product[] = [];

  constructor(
    private router: Router,
    private productService: ProductService,
    private cartService: CartService,
    private authService: AuthService,
    private renderer: Renderer2,
  ) { }

  ngOnInit() {
    window.scrollTo({ top: 0, behavior: 'smooth' });

    const navigation = this.router.getCurrentNavigation();
    this.product = navigation?.extras?.state?.['product'];

    const storedProduct = sessionStorage.getItem('selectedProduct');
    if (storedProduct) {
      this.product = JSON.parse(storedProduct);
    } else {
      // fallback API call if needed.
    }

    if(this.product){
      this.fetchSuggestedProducts(this.product.category);
    }
  }

  addToCart() {
    if(!this.authService.getToken()){
      const modalEl = document.getElementById('authModal');
      if (modalEl) {
        (modalEl as any).style.display = 'block';
        (modalEl as any).classList.add('show');
      }
      return;
    }
    const customerId = this.authService.getUserIdFromToken();
    if(!customerId) return;

    const item: CartItemDTO = {
      productId: this.product.id,
      quantity: this.productQuantity
    };

    this.cartService.addToCart(customerId, item).subscribe({
      next: () => {
        this.animateToCart(); // funzione opzionale
        // this.updateCartCount(); //TODO: funzione da implementare per aggiornare il numero accanto
        this.cartService.getCart(customerId); // to refresh the cart
        this.cartService.setCartItemCount(this.productQuantity);
      },
      error: err => {
        console.error("Errore aggiunta al carrello", err);
      }
    });
  }

  animateToCart() {
  const productImg = document.getElementById('mainProductImg');
  const cartIcon = document.getElementById('cartIcon');

  if (!productImg || !cartIcon) return;

  const clone = productImg.cloneNode(true) as HTMLElement;
  const imgRect = productImg.getBoundingClientRect();
  const cartRect = cartIcon.getBoundingClientRect();

  clone.style.position = 'fixed';
  clone.style.left = `${imgRect.left}px`;
  clone.style.top = `${imgRect.top}px`;
  clone.style.width = `${imgRect.width}px`;
  clone.style.zIndex = '1000';
  clone.style.transition = 'all 0.9s ease-in-out';

  document.body.appendChild(clone);

  requestAnimationFrame(() => {
    clone.style.left = `${cartRect.left}px`;
    clone.style.top = `${cartRect.top}px`;
    clone.style.opacity = '0.5';
    clone.style.transform = 'scale(0.2)';
  });

  setTimeout(() => { document.body.removeChild(clone); }, 1000);

  }


  incrementQuantity(){
    this.productQuantity++;
  }

  discrementQuantity(){
    if(this.productQuantity <= 1)
    {
      null;
    }
    else{ 
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
