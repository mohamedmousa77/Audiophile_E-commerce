import { Component,OnInit  } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ProductService } from '../../services/product/product.service';
import { Product } from '../../models/product';
import { CommonModule } from '@angular/common';


@Component({
  selector: 'app-category',
  imports: [CommonModule],
  templateUrl: './category.component.html',
  styleUrl: './category.component.css'
})

export class CategoryComponent implements OnInit {
  products: Product[] = [];
  categoryName: string = '';

  constructor(private route: ActivatedRoute, private productService: ProductService) {  }

  ngOnInit(): void {
    this.route.paramMap.subscribe(params => {
      this.categoryName = params.get('categoryName')?.toLowerCase() ?? '';
      this.getProducts();
    });
  }

  getProducts(): void {
    this.productService.getProductsByCategory(this.categoryName)
      .subscribe(products => {
        this.products = products;
        this.products.forEach(element => {
          if(!element.imageUrl){
            element.imageUrl = 'place-holder-3.png';
          }           
        });
      });      
  }

}
