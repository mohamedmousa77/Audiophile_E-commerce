import { Cart } from "./cart";

export interface CustomerInfo {
    id: number;
    fullName: string;
    email: string; 
    address: string;
    phone: string;
    city: string;
    country: string;
    zipCode: string;
    cart?: Cart;
}