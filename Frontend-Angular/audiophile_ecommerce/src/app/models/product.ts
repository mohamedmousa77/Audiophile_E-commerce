export interface Product {
    id: number;
    name: string;
    description: string;
    price: number;
    imageUrl: string;
    category: string;
    stockQuantity: number;
    updatedAt: Date;
    isPromotion: boolean;
    isNew: boolean;

}