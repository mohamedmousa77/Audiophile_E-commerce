import { Cart } from "./cart";
import { Product } from "./product";

export interface CartItem {
    id: number;
    cartID: number;
    productID: number;
    cart?: Cart;
    product?: Product;
    quantity: number;
}