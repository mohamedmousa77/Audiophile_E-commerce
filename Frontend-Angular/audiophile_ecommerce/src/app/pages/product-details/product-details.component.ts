import { Component } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ProductService } from '../../services/product/product.service';
import { CommonModule } from '@angular/common';

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


  constructor(
    private route: ActivatedRoute, 
    private router: Router, 
    private productS: ProductService
  ) { }

  ngOnInit() {
    window.scrollTo({ top: 0, behavior: 'smooth' });
    const navigation = this.router.getCurrentNavigation();
    this.product = navigation?.extras?.state?.['product'];
    
    console.log("Product received via router state:", this.product);
    console.log("Product received via router state:", navigation?.extras?.state?.['product']);
    
    const storedProduct = sessionStorage.getItem('selectedProduct');
    if (storedProduct) {
      this.product = JSON.parse(storedProduct);
    } else {
      // fallback API call
    }
  console.log("Loaded product:", this.product);
    
    // if (!this.product) {
    //   // fallback
    //   this.productId = parseInt(this.route.snapshot.paramMap.get('id')!);
    //   this.product = this.productS.getProductById(this.productId);
    //   console.warn("Product not found in navigation state. ID from URL:", this.productId);
    // } else {
    //   console.log("Product received via router state:", this.product);
    // }
  }

}
