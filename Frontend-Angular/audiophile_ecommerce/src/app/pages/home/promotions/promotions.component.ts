import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';

@Component({
  selector: 'app-promotions',
  imports: [CommonModule],
  templateUrl: './promotions.component.html',
  styleUrl: './promotions.component.css'
})
export class PromotionsComponent {

  promotionsProducts = [
    {
      id: 0,
      title: 'ZX9 SPEAKER',
      description: 'Upgrade to premium speakers that are phenomenally built to deliver truly remarkable sound.',
      imageUrl: '3D_Speaker-image.png',
      alt: 'ZX9 Speaker',
      link: '/product/zx9'
    },
    {
      id: 1,
      title: 'ZX7 SPEAKER',
      imageUrl: 'speacker-img.png',
      link: '/product/zx7-speaker',
      alt: 'ZX9 Speaker',
    },
    {
      id:2,
      title: 'YX1 EARPHONES',
      imageUrl: 'earphone-cat-img-bg.png',
      link: '/product/yx1-earphones',
      alt: 'ZX9 Speaker',
    }
  ];


  constructor(private router: Router) {  }
  
  viewProduct(productId: number) {
    this.router.navigate(['/product', productId], {
    state: { product: this.promotionsProducts[productId]  }
  });
  }

}
