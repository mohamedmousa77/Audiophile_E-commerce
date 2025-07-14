import { CustomerInfo } from "./customerInfo";
import { OrderItem } from "./orderItem";

export interface Order {
    id: number;
    customerInfoId: number;
    customerInfo: CustomerInfo;
    items: OrderItem[];

    subTotal: number;
    shipping: number;
    vat: number;
    total: number;

    status: string;
    createdAt: Date;
    updatedAt: Date;
}