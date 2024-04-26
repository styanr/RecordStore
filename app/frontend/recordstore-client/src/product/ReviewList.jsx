import { Box, Text, VStack, HStack, Flex, Button } from '@chakra-ui/react';
import formatDate from '../utils/formatDate';

import { StarIcon } from '@chakra-ui/icons';

const ReviewList = ({ reviews }) => (
  <VStack spacing={4} align='stretch'>
    {reviews.length > 0 ? (
      reviews.map((review) => (
        <Box
          key={review.id}
          p={5}
          borderWidth={1}
          borderRadius='md'
          boxShadow='lg' // Subtle shadow for depth
          bg='white' // Light background color
          _hover={{
            // Hover effect for interactivity
            boxShadow: 'xl',
            transform: 'scale(1.02)',
          }}
          transition={'transform 0.2s'}
        >
          <Flex justifyContent='space-between' alignItems='center' mb={3}>
            <Text fontWeight='bold' fontSize='lg'>
              {review.userFullName}
            </Text>
            <Text fontSize='xs' color='gray.500'>
              {formatDate(review.createdAt)}
            </Text>
          </Flex>
          <HStack spacing={1} mb={3}>
            {[1, 2, 3, 4, 5].map((rating) => (
              <StarIcon
                key={rating}
                color={rating <= review.rating ? 'yellow.500' : 'gray.300'}
                boxSize={5}
              />
            ))}
          </HStack>
          <Text fontSize='sm' color='gray.700' lineHeight='tall'>
            {review.description}
          </Text>
        </Box>
      ))
    ) : (
      <Text color='gray.500'>
        Наразі немає жодного відгуку для цього товару.
      </Text>
    )}
  </VStack>
);

export default ReviewList;
