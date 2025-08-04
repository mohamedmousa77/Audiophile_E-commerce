import { Directive, ElementRef, Input, Renderer2 } from '@angular/core';

@Directive({
  selector: '[appFlyToCart]'
})
export class FlyToCartDirective {
  @Input() cartIconId!: string;

  constructor(private el: ElementRef, private renderer: Renderer2) { }

  public fly() {
    const productImg = this.el.nativeElement;
    const cartIcon = document.getElementById(this.cartIconId);
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
}
