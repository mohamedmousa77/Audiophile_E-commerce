import { Component } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ProductService } from '../../services/product/product.service';
import { CommonModule } from '@angular/common';
import { Product } from '../../models/product';

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

  suggestedProducts = [
  {
    name: 'ZX7 Speaker',
    imageUrl: '3D_Speaker-image.png',
    shortDescription: 'Compact wireless speaker with premium sound.',
  },
  {
    name: 'YX1 Earphones',
    imageUrl: '3D_Speaker-image.png',
    shortDescription: 'High-performance earphones for daily use.',
  },
  {
    name: 'XX99 Headphones',
    imageUrl: '3D_Speaker-image.png',
    shortDescription: 'Studio quality headphones with noise cancelling.',
  },
  {
    name: 'YX1 Earphones',
    imageUrl: '3D_Speaker-image.png',
    shortDescription: 'High-performance earphones for daily use.',
  },
  {
    name: 'XX99 Headphones',
    imageUrl: '3D_Speaker-image.png',
    shortDescription: 'Studio quality headphones with noise cancelling.',
  }
];
  productsSuggested: Product[] = [];

  constructor(
    private router: Router,
    private productService: ProductService
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
