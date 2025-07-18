import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
@Component({
  selector: 'app-promotions',
  imports: [CommonModule],
  templateUrl: './promotions.component.html',
  styleUrl: './promotions.component.css'
})
export class PromotionsComponent {
  zx9Product = {
    title: 'ZX9 SPEAKER',
    description: 'Upgrade to premium speakers that are phenomenally built to deliver truly remarkable sound.',
    imageUrl: '3D_Speaker-image.png',
    alt: 'ZX9 Speaker',
    link: '/product/zx9'
  };
  zx7Product = {
    title: 'ZX7 SPEAKER',
    imageUrl: 'speacker-img.png',
    link: '/product/zx7-speaker',
    alt: 'ZX9 Speaker',
  };

  yx1Product = {
    title: 'YX1 EARPHONES',
    imageUrl: 'earphone-cat-img-bg.png',
    link: '/product/yx1-earphones',
    alt: 'ZX9 Speaker',
  };

  promotionsProducts = [
    {
      title: 'ZX9 SPEAKER',
      description: 'Upgrade to premium speakers that are phenomenally built to deliver truly remarkable sound.',
      imageUrl: '3D_Speaker-image.png',
      alt: 'ZX9 Speaker',
      link: '/product/zx9'
    },
    {
      title: 'ZX7 SPEAKER',
      imageUrl: 'speacker-img.png',
      link: '/product/zx7-speaker',
      alt: 'ZX9 Speaker',
    },
    {
      title: 'YX1 EARPHONES',
      imageUrl: 'earphone-cat-img-bg.png',
      link: '/product/yx1-earphones',
      alt: 'ZX9 Speaker',
    }
  ]

}
