import { Component } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ProductService } from '../../services/product/product.service';
import { CommonModule } from '@angular/common';
import { Product } from '../../models/product';
import { CartService } from '../../services/cart/cart.service';
import { CartItemDTO } from '../../models/cartItemDTO';

@Component({
  selector: 'app-product-details',
  imports: [CommonModule],
  templateUrl: './product-details.component.html',
  styleUrl: './product-details.component.css'
})

export class ProductDetailsComponent {

  productId: number = 0;
  product: any;
  companyName = 'Audiophile';
  productQuantity = 1;

//   suggestedProducts = [
//   {
//     name: 'ZX7 Speaker',
//     imageUrl: '3D_Speaker-image.png',
//     shortDescription: 'Compact wireless speaker with premium sound.',
//   },
//   {
//     name: 'YX1 Earphones',
//     imageUrl: '3D_Speaker-image.png',
//     shortDescription: 'High-performance earphones for daily use.',
//   },
//   {
//     name: 'XX99 Headphones',
//     imageUrl: '3D_Speaker-image.png',
//     shortDescription: 'Studio quality headphones with noise cancelling.',
//   },
//   {
//     name: 'YX1 Earphones',
//     imageUrl: '3D_Speaker-image.png',
//     shortDescription: 'High-performance earphones for daily use.',
//   },
//   {
//     name: 'XX99 Headphones',
//     imageUrl: '3D_Speaker-image.png',
//     shortDescription: 'Studio quality headphones with noise cancelling.',
//   }
// ];
  productsSuggested: Product[] = [];

  constructor(
    private router: Router,
    private productService: ProductService,
    private cartService: CartService
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
    const customerId = 1;
    // this.authService.getUserId(); // oppure leggi da localStorage
    const productId = this.product.id;
    const quantity = 1; // oppure leggi da input quantità

    const item: CartItemDTO = {
      productId: productId,
      quantity: quantity
    };

    this.cartService.addToCart(customerId, item).subscribe({
      next: () => {
        this.animateToCart(); // funzione opzionale
        // this.updateCartCount(); //TODO: funzione da implementare per aggiornare il numero accanto
      },
      error: err => {
        console.error("Errore aggiunta al carrello", err);
      }
    });
  }

  animateToCart() {
    const img = document.getElementById('mainProductImg');
    const cartIcon = document.querySelector('.cart-icon img');//TODO: check this cart-icon

    if (!img || !cartIcon) return;

    const imgRect = img.getBoundingClientRect();
    const cartRect = cartIcon.getBoundingClientRect();

    const clone = img.cloneNode(true) as HTMLElement;
    clone.style.position = 'fixed';
    clone.style.left = `${imgRect.left}px`;
    clone.style.top = `${imgRect.top}px`;
    clone.style.width = `${imgRect.width}px`;
    clone.style.transition = 'all 0.8s ease-in-out';
    clone.style.zIndex = '9999';

    document.body.appendChild(clone);

    setTimeout(() => {
      clone.style.left = `${cartRect.left}px`;
      clone.style.top = `${cartRect.top}px`;
      clone.style.width = '30px';
      clone.style.opacity = '0.5';
    }, 10);

    setTimeout(() => {
      document.body.removeChild(clone);
    }, 1000);
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
