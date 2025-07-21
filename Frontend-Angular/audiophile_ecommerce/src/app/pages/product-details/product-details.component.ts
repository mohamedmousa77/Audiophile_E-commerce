import { Component } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-product-details',
  imports: [],
  templateUrl: './product-details.component.html',
  styleUrl: './product-details.component.css'
})
export class ProductDetailsComponent {

  productId: number = 0;

  constructor(private route: ActivatedRoute) {  }

  ngOnInit() {
    this.productId = +this.route.snapshot.paramMap.get('id')!;
  }

}
