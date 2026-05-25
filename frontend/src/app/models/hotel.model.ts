import { Room } from './room.model';

export interface Location {
  id: number;
  name: string;
}

export interface Amenity {
  id: number;
  name: string;
  icon: string;
}

export interface HotelAmenity {
  hotelId: number;
  amenityId: number;
  amenity?: Amenity;
}

export interface Hotel {
  id: number;
  name: string;
  locationId: number;
  location?: Location;
  description: string;
  imageUrl: string;
  rating: number;
  rooms?: Room[];
  hotelAmenities?: HotelAmenity[];
}
