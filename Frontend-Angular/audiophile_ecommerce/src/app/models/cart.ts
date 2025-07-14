import { CartItem } from "./cartItem";
import { CustomerInfo } from "./customerInfo";

export interface Cart {
    id: number;
    customerInfoId: number;
    customerInfo?: CustomerInfo;
    items: CartItem [];
}