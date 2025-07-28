import { Component, AfterViewInit, OnInit} from '@angular/core';
import { gsap } from 'gsap';
import {ScrollTrigger} from 'gsap/ScrollTrigger';
import { CommonModule } from '@angular/common';

import { PromotionsComponent } from './promotions/promotions.component';
import { AboutComponent } from './about/about.component';

import { Router } from '@angular/router';
import { Product } from '../../models/product';
import { ProductService } from '../../services/product/product.service';

@Component({
  selector: 'app-home',
  imports: [CommonModule, PromotionsComponent, AboutComponent ],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent implements OnInit {
// implements AfterViewInit
  categories = [
    {title: 'HEADPHONES', image:'headphones-image.png', },
    {title: 'SPEAKERS', image:'3D_Speaker-image.png', },
    {title: 'EARPHONES', image:'earphones-image.png', },
  ]

  newProduct?: Product;

  constructor(private router: Router, private productService: ProductService) {}

  ngOnInit() {
    this.productService.getFilteredProducts(false, true)
    .subscribe(products => {
      
      products.forEach(element => {
      element.isNew? this.newProduct = element : null ;
      console.log(`new product fetched correctly: product: ${this.newProduct?.name}`)
      });
    });
  }

  navigateToCategory(category: string): void {
    this.router.navigate(['/category', category.toLowerCase()]);
  }

  viewProduct() {
    sessionStorage.setItem('selectedProduct', JSON.stringify(this.newProduct));    
    this.router.navigate(['/product', this.newProduct!.id]);
  }

}
