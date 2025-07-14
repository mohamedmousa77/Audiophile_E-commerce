import { Order } from "./order";
import { Product } from "./product";

export interface OrderItem {
    id: number;
    productId: number;
    orderId: number;
    product: Product;
    order: Order;
    quantity: number;
    unitPrice: number;
}