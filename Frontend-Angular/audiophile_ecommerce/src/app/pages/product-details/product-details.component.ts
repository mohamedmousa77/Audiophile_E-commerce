import { Component } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Product } from '../../models/product';

@Component({
  selector: 'app-product-details',
  imports: [],
  templateUrl: './product-details.component.html',
  styleUrl: './product-details.component.css'
})
export class ProductDetailsComponent {

  productId: number = 0;
  product: any;

  constructor(private route: ActivatedRoute, private router: Router) {  }

  ngOnInit() {
    const navigation = this.router.getCurrentNavigation();
    this.product = navigation?.extras?.state?.['product'];

    if (!this.product) {
      // fallback: load by ID
      const id = this.route.snapshot.paramMap.get('id');
      // chiamata API per prendere i dati con quell'id
    }
  }

}
